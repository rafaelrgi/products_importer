import { Injectable } from '@nestjs/common';
import { Product } from './entities/product';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { IPaginatedDto } from 'src/core/dto/paginated.dto';
import { IQueryParams } from './dto/query-params.dto';

@Injectable()
export class ProductService {
  //
  constructor(
    @InjectRepository(Product)
    private productRepository: Repository<Product>,
  ) { }

  findAll(): Promise<Product[]> {
    return this.productRepository.find();
  }

  async paginated(options: IQueryParams): Promise<IPaginatedDto<Product>> {
    const skip = (options.page - 1) * options.perPage;

    const query = this.productRepository
      .createQueryBuilder('Product')
      .skip(skip)
      .take(options.perPage);

    //sorting based on the name, price, and expiration fields
    if (options.sort) {
      query.orderBy(options.sort, options.order as 'ASC' | 'DESC' | undefined);
    }

    //filtering based on the name, price, and expiration fields
    if (options.name) {
      query.where('name LIKE :name', { name: `%${options.name}%` });
    }
    if (options.priceMin) {
      query.andWhere('price >= :priceMin', { priceMin: options.priceMin });
    }
    if (options.priceMax) {
      query.andWhere('price <= :priceMax', { priceMax: options.priceMax });
    }
    if (options.expirationMin) {
      query.andWhere('expiration >= :expirationMin', {
        expirationMin: options.expirationMin,
      });
    }
    if (options.expirationMax) {
      query.andWhere('expiration <= :expirationMax', {
        expirationMax: options.expirationMax,
      });
    }

    const [data, count] = await query.getManyAndCount();
    const result: IPaginatedDto<Product> = {
      meta: {
        page: options.page,
        perPage: options.perPage,
        recordCount: count,
        pageCount: Math.ceil(count / options.perPage),
      },
      data: data,
    };

    return result;
  }
}
