import { GridCellDefaultProps } from '@/components/Grid';

export interface MaskSensitiveValueOptions {
	maskSymbol?: string;
	maskRepeatCount?: number;
}

export interface HiddenValueCellRendererOptions<
	T extends GridCellDefaultProps
> {
	fieldToDisplay?: keyof T;
	options?: MaskSensitiveValueOptions;
}
