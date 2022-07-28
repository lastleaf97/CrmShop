using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmBl.Model
{
    public class CashDesk // класс кассы
    {
        CrmContext db; // контекст БД
        public int Number { get; set; } // номер кассы
        public Seller Seller { get; set; } // продавец
        public Queue<Cart> Queue { get; set; } // очередь
        public int MaxQueueLenght { get; set; } // максимальная длина очереди
        public int ExitCustomer { get; set; } // счетчик
        public bool IsModel { get; set; } // будет ли выполняться сохранение в БД
        public int Count => Queue.Count; // определение наименьшей очереди
        public event EventHandler<Check> CheckClosed; // событие продажи
        public CashDesk(int number, Seller seller, CrmContext db)
        {
            Number = number;
            Seller = seller;
            Queue = new Queue<Cart>();
            IsModel = true;
            MaxQueueLenght = 10;
            this.db = db ?? new CrmContext();
        }

        public void Enqueue(Cart cart) // метод добавления человека в очередь
        {
            if (Queue.Count < MaxQueueLenght)
            {
                Queue.Enqueue(cart);
            }
            else
            {
                ExitCustomer++;
            }
        }
        public decimal Dequeue() // метод исключения человека из очереди
        {
            decimal sum = 0;

            if (Queue.Count == 0)
            {
                return 0;
            }

            var card = Queue.Dequeue();
            if(card != null)
            {
                var check = new Check()
                {
                    SellerId = Seller.SellerId,
                    Seller = Seller,
                    CustomerId = card.Customer.CustomerId,
                    Customer = card.Customer,
                    Created = DateTime.Now,
                };

                if (!IsModel)
                {
                    db.Checks.Add(check);
                    db.SaveChanges();
                }
                else
                {
                    check.CheckId = 0;
                }

                var sells = new List<Sell>(); // список покупок
                foreach (Product product  in card) // перебор элементов корзины
                {
                    if (product.Count > 0)
                    {
                        var sell = new Sell()
                        {
                            CheckId = check.CheckId,
                            Check = check,
                            ProductId = product.ProductId,
                            Product = product
                        };

                        sells.Add(sell);

                        if (!IsModel)
                        {
                            db.Sells.Add(sell);
                        }

                        product.Count--;
                        sum += product.Price;
                    }
                }

                check.Price = sum;

                if (!IsModel)
                {
                    db.SaveChanges();
                }

                CheckClosed?.Invoke(this, check); // генерация события продажи
            }

            return sum;
        }

        public override string ToString()
        {
            return $"Касса №{Number}";
        }
    }
}
