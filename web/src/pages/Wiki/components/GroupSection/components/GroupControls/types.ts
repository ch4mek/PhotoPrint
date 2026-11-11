import { ChangeEvent } from 'react';

import { SegmentTypes } from '@/types/wiki';

export interface GroupControlsProps {
	handleSearchChange: (event: ChangeEvent<HTMLInputElement>) => void;
	searchValue: string;
	handleAddGroupClick: (
		type: SegmentTypes,
		parentGroupId?: string | number
	) => () => void;
	isMoveModeEnable: boolean;
	handleModeButtonClick: VoidFunction;
	handleSearchReset: VoidFunction;
	isButtonsDisabled: boolean;
}
