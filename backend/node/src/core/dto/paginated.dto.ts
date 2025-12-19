export interface IPaginatedDto<T> {
  meta: {
    page: number;
    perPage: number;
    recordCount: number;
    pageCount: number;
  };
  data: T[];
}
