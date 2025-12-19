export interface IQueryParams {
  //pagination
  page: number;
  perPage: number;
  //sorting
  sort: string;
  order: string;
  //filtering
  name: string;
  priceMin: number | null;
  priceMax: number | null;
  expirationMin: Date | null;
  expirationMax: Date | null;
}
