import { SortDirection } from '@/constants/sort';

export const DEFAULT_ITEMS_PER_PAGE = 15;
const CREATED_DATE_FIELD = 'createdAtUtc';
export const DEFAULT_ORDER_BY = `${CREATED_DATE_FIELD} ${SortDirection.DESC}`;
