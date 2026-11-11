import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';

export interface TileListItemProps<T> {
	item: T;
	onClick?: (item: T) => void;
	isSelected: boolean;
	top: number;
	getContextMenuOptions?: (item: T) => PickerButtonOption<T>[];
	index: number;
	fieldToDisplay: keyof T;
	isPointerCursor: boolean;
}
