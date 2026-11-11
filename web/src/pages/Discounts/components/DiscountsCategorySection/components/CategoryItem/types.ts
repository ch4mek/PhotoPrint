import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { MouseEvent } from 'react';

import { MenuItem } from '@/components/DraggableControlIcon/types';
import { DiscountCategory } from '@/types/discountCategories';

export interface CategoryItemProps {
	item: DiscountCategory;
	handleClick: (id: number | string) => (e: MouseEvent<HTMLDivElement>) => void;
	isSelectedItem: boolean;
	isReadMode?: boolean;
	menuOptions?: PickerButtonOption<MenuItem>[];
	isMoveMode?: boolean;
}
