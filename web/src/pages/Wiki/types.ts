export enum DropIndicatorPositionType {
	Top = 'top',
	Bottom = 'bottom',
	Middle = 'middle',
	None = 'none',
}

export interface TreeNodes {
	items: Option[];
}

interface BasicOptions {
	id: string | number;
	isGroup: boolean;
	name: string;
	orderWeight: number;
	parentGroupId: null | number | string;
	expanded?: boolean;
}

export interface Option extends BasicOptions {
	accessPath: string;
	items?: Option[];
}

export interface OptionWithoutPath extends BasicOptions {
	items?: OptionWithoutPath[];
}
