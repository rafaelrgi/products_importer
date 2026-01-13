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

/*
{
  hasData = true
  page =1
  pageCount =248
  perPage =15
  recordCount =3718
  data[{
    id =1
    name ='Calypso - Lemonade #(4026987913289674)'
    expiration ='2023-11-01T00:00:00'
    quantity =0
    price =115.55
    brl =640.62
    cad =158.05
    eur =98.14
    mxn =2069.48
    gbp =85.62
    isDeleted =false
    createdAt ='2025-12-29T00:03:24.834303'
    updatedAt ='2025-12-29T00:03:41.100332'
    deletedAt =null
  }]
}
*/