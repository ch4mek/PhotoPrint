import { Theme } from '@emotion/react';
import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { RefObject } from 'react';

import { SkillsNames } from '../../constants';

export interface TileListProps<T extends { id: string | number }> {
	items: Array<T>;
	totalItems: number;
	showLoader?: boolean;
	isLoading?: boolean;
	page?: number;
	setPage?: (nameOfPageSkill: SkillsNames, newPage: number) => void;
	onAddClick?: () => void;
	title: SkillsNames;
	beforeColor: keyof Theme['palette'];
	onSelect?: (item: T) => void;
	selectedIndex?: number | string | null;
	fieldToDisplay: keyof T;
	getListItemContextMenuOption?: (item: T) => PickerButtonOption<T>[];
	isPointerCursor?: boolean;
	containerRef?: RefObject<HTMLDivElement | null>;
}
