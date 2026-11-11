import { BYTES_IN_MB } from '@/constants/file';

export const convertBytesToMB = (size?: number) =>
	((size ?? 0) / BYTES_IN_MB).toFixed(2);
