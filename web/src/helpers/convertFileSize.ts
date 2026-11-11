import { BYTES_IN_KB } from '@/constants/file';

export const convertKBToMB = (fileSizeInMB: number) =>
	fileSizeInMB / BYTES_IN_KB;

export const convertBytesToKB = (fileSizeInBytes: number) =>
	fileSizeInBytes / BYTES_IN_KB;
