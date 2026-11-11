export enum FilterTagType {
	Main = 'main',
	Secondary = 'secondary',
}

export interface FilterTagProps {
	id: number;
	title: string;
	isSelected: boolean;
	disabled: boolean;
	onClick: (id: number) => void;
	type?: FilterTagType;
}

export interface FilterTagWrapperProps {
	isSelected: boolean;
	type: FilterTagType;
	disabled: boolean;
}
