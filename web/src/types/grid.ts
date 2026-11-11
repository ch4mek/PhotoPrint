import { ColumnWidthConfig } from '@/components/Grid/types';

export type GridColumnWidth = ColumnWidthConfig | null | number | string;

export type GridColumnWidthsConfiguration<T = string> = T extends object
	? {
			[key in keyof T]: GridColumnWidth;
	  }
	: {
			[key: string]: GridColumnWidth;
	  };

export type GridColumnsNamesConfiguration<T> = {
	[key in keyof T]: {
		key: keyof T;
		name: string;
	};
};

export type HandleRowClick<T> = (data: T) => void;
