import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton/types';

interface SelectedItem {
	id: number | string;
	parentGroupId?: null | number;
	name: string;
}

export interface DraggableControlIconProps {
	menuOptions: PickerButtonOption<MenuItem>[];
	isMoveMode: boolean;
	item: SelectedItem;
	withIconSpace?: boolean;
	iconSize?: 'small' | 'large';
	isDisablePickerButton?: boolean;
}

export interface MenuItem {
	id: number;
	name: string;
}
