export interface Product {
  id: number;
  name: string;

  expiration: Date;
  quantity: number;
  price: number;

  brl: number;
  cad: number;
  eur: number;
  mxn: number;
  gbp: number;

  isDeleted: boolean;
  createdAt: Date;
  updatedAt: Date;
}