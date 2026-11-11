import { TimePrefix } from '@/constants/time';

export const generateTimeValue = (value: number, prefix: TimePrefix) =>
	`${value} ${prefix}`;
