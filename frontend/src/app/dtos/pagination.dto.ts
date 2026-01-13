export class Pagination {
  constructor(response: any) {
    this.hasData = response.hasData;
    this.page = response.page;
    this.pageCount = response.pageCount;
    this.perPage = response.perPage;
    this.recordCount = response.recordCount;
  }

  public hasData: boolean = false;
  public page: number = 0;
  public pageCount: number = 0;
  public perPage: number = 0;
  public recordCount: number = 0;
}
/*

export interface Pagination {
  hasData: boolean;
  page: number;
  pageCount: number;
  perPage: number;
  recordCount: number;
}
*/