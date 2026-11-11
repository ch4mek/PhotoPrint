import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { MouseEventHandler } from 'react';

import { MenuItem } from '@/components/DraggableControlIcon/types';
import { Option } from '@/pages/Wiki/types';

export interface StaticItemProps {
	onClick?: MouseEventHandler;
	isSelected: boolean;
	menuOptions: PickerButtonOption<MenuItem>[];
	isMoveMode: boolean;
	item: Option;
	disabled: boolean;
}
