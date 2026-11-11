import { useState } from 'react';

import { CALENDAR_ADD_BUTTON_TOOLTIP_MESSAGE } from '../../constants';

import { ButtonAdd } from '@/components/ButtonAdd';
import { isHeadOrGroupManager } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';

import { AddButtonCellWrapper } from './styles';
import { AddButtonCellProps } from './types';

export function AddButtonCell({ onAddClick, disabled }: AddButtonCellProps) {
	const [isLoading, setIsLoading] = useState<boolean>(false);
	const { role } = useAuth();
	const onLoadingEnd = () => {
		setIsLoading(false);
	};

	const handleAddClick = () => {
		setIsLoading(true);
		onAddClick(onLoadingEnd);
	};

	const isDisabled = disabled || isLoading;

	return (
		<AddButtonCellWrapper>
			<ButtonAdd
				disabled={isDisabled}
				onAdd={handleAddClick}
				tooltipMessage={CALENDAR_ADD_BUTTON_TOOLTIP_MESSAGE}
				showTooltip={!isHeadOrGroupManager(role) && isDisabled}
			/>
		</AddButtonCellWrapper>
	);
}
