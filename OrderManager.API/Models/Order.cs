using OrderManager.API.DTO;
using OrderManager.API.Validations;

namespace OrderManager.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = [];

        public ValidationResult AddPositions(List<OrderItemDTO> positions, Dictionary<int, Product> productsDict)
        {
            var notFoundPositions = new List<int>();
            var notAvailablePositions = new List<int>();
            foreach (var position in positions)
            {
                if (!productsDict.TryGetValue(position.ProductId, out var product))
                {
                    notFoundPositions.Add(position.ProductId);
                    continue;
                }

                var result = AddPositionInternal(position, product);
                switch (result)
                {
                    case PositionResult.Success:
                        continue;
                    case PositionResult.NotAvailable:
                        notAvailablePositions.Add(position.ProductId);
                        break;
                    case PositionResult.NotFound:
                        notFoundPositions.Add(position.ProductId);
                        break;
                    default:
                        return ValidationResult.FailureResult(OrderErrorMessages.InvalidPositions());
                }
            }

            if (notFoundPositions.Count > 0)
            {
                return ValidationResult.FailureResult(OrderErrorMessages.PositionsNotFound(Id, notFoundPositions));
            }

            if (notAvailablePositions.Count > 0)
            {
                return ValidationResult.FailureResult(ProductErrorMessages.ProductsNotAvailable(notAvailablePositions));
            }

            return ValidationResult.SuccessResult();
        }

        public ValidationResult AddPosition(OrderItemDTO position, Product product)
        {
            return AddPositionInternal(position, product) switch
            {
                PositionResult.Success => ValidationResult.SuccessResult(),
                PositionResult.NotFound => ValidationResult.FailureResult(OrderErrorMessages.PositionNotFound(Id, position.ProductId)),
                PositionResult.NotAvailable => ValidationResult.FailureResult(ProductErrorMessages.ProductNotAvailable(product.Id)),
                _ => ValidationResult.FailureResult(OrderErrorMessages.InvalidPosition())
            };
        }

        public ValidationResult RemovePositions(List<int> positionsIds, Dictionary<int, Product> productsDict)
        {
            var notFoundPositions = new List<int>();
            foreach (var positionId in positionsIds)
            {
                if (!productsDict.TryGetValue(positionId, out var product))
                {
                    notFoundPositions.Add(positionId);
                    continue;
                }

                var result = RemovePositionInternal(positionId, product);
                switch (result)
                {
                    case PositionResult.Success:
                        continue;
                    case PositionResult.NotFound:
                        notFoundPositions.Add(positionId);
                        break;
                    default:
                        return ValidationResult.FailureResult(OrderErrorMessages.InvalidPositions());
                }
            }

            if (notFoundPositions.Count > 0)
            {
                return ValidationResult.FailureResult(OrderErrorMessages.PositionsNotFound(Id, notFoundPositions));
            }

            return ValidationResult.SuccessResult();
        }

        public ValidationResult RemovePosition(int productId, Product product)
        {
            return RemovePositionInternal(productId, product) switch
            {
                PositionResult.Success => ValidationResult.SuccessResult(),
                PositionResult.NotFound => ValidationResult.FailureResult(OrderErrorMessages.PositionNotFound(Id, productId)),
                _ => ValidationResult.FailureResult(OrderErrorMessages.InvalidPosition())
            };
        }

        public ValidationResult ModifyPostions(IEnumerable<OrderItemDTO> positions, Dictionary<int, Product> productsDict)
        {
            var notFoundPositions = new List<int>();
            var notAvailablePositions = new List<int>();
            foreach (var position in positions)
            {
                if (!productsDict.TryGetValue(position.ProductId, out var product))
                {
                    notFoundPositions.Add(position.ProductId);
                    continue;
                }

                var result = ModifyPositionInternal(position, product);
                switch (result)
                {
                    case PositionResult.Success:
                        continue;
                    case PositionResult.NotFound:
                        notFoundPositions.Add(position.ProductId);
                        break;
                    case PositionResult.NotAvailable:
                        notAvailablePositions.Add(position.ProductId);
                        break;
                    default:
                        return ValidationResult.FailureResult(OrderErrorMessages.InvalidPositions());
                }
            }

            if (notFoundPositions.Count > 0)
            {
                return ValidationResult.FailureResult(OrderErrorMessages.PositionsNotFound(Id, notFoundPositions));
            }

            if (notAvailablePositions.Count > 0)
            {
                return ValidationResult.FailureResult(ProductErrorMessages.ProductsNotAvailable(notAvailablePositions));
            }

            return ValidationResult.SuccessResult();
        }

        public ValidationResult ModifyPosition(OrderItemDTO position, Product product)
        {
            return ModifyPositionInternal(position, product) switch
            {
                PositionResult.Success => ValidationResult.SuccessResult(),
                PositionResult.NotFound => ValidationResult.FailureResult(OrderErrorMessages.PositionNotFound(Id, product.Id)),
                PositionResult.NotAvailable => ValidationResult.FailureResult(ProductErrorMessages.ProductNotAvailable(product.Id)),
                PositionResult.Invalid => ValidationResult.FailureResult(OrderErrorMessages.InvalidPosition()),
                _ => ValidationResult.FailureResult(OrderErrorMessages.InvalidPosition())
            };
        }

        private PositionResult AddPositionInternal(OrderItemDTO position, Product product)
        {
            if (position is null)
            {
                return PositionResult.Invalid;
            }

            if (product is null)
            {
                return PositionResult.NotFound;
            }

            if (!product.DecreaseStock(position.Quantity))
            {
                return PositionResult.NotAvailable;
            }


            var orderItem = OrderItems.FirstOrDefault(oi => oi.ProductId == position.ProductId);
            if (orderItem is null)
            {
                orderItem = new OrderItem
                {
                    ProductId = position.ProductId,
                    Quantity = position.Quantity,
                    Price = product.Price,
                    Product = product
                };
                OrderItems.Add(orderItem);
            }
            else
            {
                orderItem.Quantity += position.Quantity;
            }

            TotalPrice += orderItem.Price * position.Quantity;
            return PositionResult.Success;
        }

        private PositionResult RemovePositionInternal(int productId, Product product)
        {
            if (product is null)
            {
                return PositionResult.Invalid;
            }

            var orderItems = OrderItems.Where(oi => oi.ProductId == productId).ToList();
            if (orderItems.Count == 0)
            {
                return PositionResult.NotFound;
            }

            foreach (var orderItem in orderItems)
            {
                OrderItems.Remove(orderItem);
                TotalPrice -= orderItem.Price * orderItem.Quantity;
                product.IncreaseStock(orderItem.Quantity);
            }

            return PositionResult.Success;
        }

        private PositionResult ModifyPositionInternal(OrderItemDTO position, Product product)
        {
            if (position is null)
            {
                return PositionResult.Invalid;
            }

            if (product is null)
            {
                return PositionResult.NotFound;
            }

            var orderItem = OrderItems.FirstOrDefault(oi => oi.ProductId == position.ProductId);
            if (orderItem is null)
            {
                return PositionResult.NotFound;
            }

            var quantityDifference = position.Quantity - orderItem.Quantity;
            if (quantityDifference == 0)
            {
                return PositionResult.Success;
            }

            if (quantityDifference > 0)
            {
                if (!product.DecreaseStock(quantityDifference))
                {
                    return PositionResult.NotAvailable;
                }
            }
            else
            {
                product.IncreaseStock(-quantityDifference);
            }

            TotalPrice += orderItem.Price * quantityDifference;
            orderItem.Quantity = position.Quantity;
            return PositionResult.Success;
        }

        private enum PositionResult
        {
            Success, NotAvailable, NotFound, Invalid
        }
    }
}
