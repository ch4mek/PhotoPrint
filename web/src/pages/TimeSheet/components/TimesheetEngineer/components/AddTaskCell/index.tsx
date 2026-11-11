import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useContext, useMemo, useState } from 'react';
import { RenderCellProps } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { TimeSheetContext } from '../..';
import { addButtonTooltipFallbackPlacements } from '../../constants';
import { AddButtonWrapper } from '../../styles';
import { TimeSheetRow } from '../../types';

import { ButtonAdd } from '@/components/ButtonAdd';

export function AddTaskCell(props: RenderCellProps<TimeSheetRow, unknown>) {
	const { t } = useTranslation('common', { keyPrefix: 'tooltips' });
	const { row } = props;
	const {
		entireRowHoverMessage,
		timeMap,
		projectName,
		isEditingForbiddenForPeriod,
	} = row;
	const { onAddNewRow, isDisabledTimesheet, displayTooltips } =
		useContext(TimeSheetContext);
	const [isHovered, setIsHovered] = useState<boolean>(false);

	const isTrackedTime = Array.from(timeMap.values()).find(
		(item) =>
			item !== undefined &&
			(item.factTrackedInMinutes !== 0 ||
				(item.billableTrackedInMinutes !== null &&
					item.billableTrackedInMinutes !== 0))
	);

	const handleAddNewTask = () => {
		onAddNewRow();
		setIsHovered(false);
	};

	const handleMouseEnter = () => setIsHovered(true);
	const handleMouseLeave = () => setIsHovered(false);

	const isAddButtonDisabled =
		!projectName || !isTrackedTime || isDisabledTimesheet;

	const tooltipIsOpen = useMemo(
		() =>
			!isDisabledTimesheet &&
			displayTooltips &&
			isHovered &&
			isAddButtonDisabled &&
			!isEditingForbiddenForPeriod,
		[
			isDisabledTimesheet,
			displayTooltips,
			isHovered,
			isAddButtonDisabled,
			isEditingForbiddenForPeriod,
		]
	);
	const tooltipText = entireRowHoverMessage || t('firstNeedTrackHours');

	return (
		<AddButtonWrapper>
			<Tooltip
				centered={false}
				position="bottom-start"
				open={tooltipIsOpen}
				text={tooltipText}
				fallbackPlacements={addButtonTooltipFallbackPlacements}
				preventOverflow={false}
			>
				<div onMouseEnter={handleMouseEnter} onMouseLeave={handleMouseLeave}>
					<ButtonAdd disabled={isAddButtonDisabled} onAdd={handleAddNewTask} />
				</div>
			</Tooltip>
		</AddButtonWrapper>
	);
}
