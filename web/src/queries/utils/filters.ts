import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';

export const containsFilter = (field: string, value: string): string =>
	`contains(tolower(${field}), tolower('${replaceSpecialCharacters(value)}'))`;

export const orFilter = (filters: string[]): string =>
	`(${filters.join(' or ')})`;

export const andFilter = (filters: string[]): string =>
	`(${filters.join(' and ')})`;
