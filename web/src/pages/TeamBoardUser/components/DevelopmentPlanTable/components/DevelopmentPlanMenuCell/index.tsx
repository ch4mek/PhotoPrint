import { Button } from '@mms/mms-ui-library/components/Button';
import {
	PickerButton,
	PickerButtonOption,
} from '@mms/mms-ui-library/components/PickerButton';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { useMemo, useRef } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { controlsText } from '@/constants/controlsTexts';
import { useAddScrollListener } from '@/hooks/useAddScrollListener';
import {
	DevelopmentPlanRow,
	DevelopmentPlanStatus,
} from '@/types/developmentPlan';

const { editText, rejectText, approveText, deleteText } = controlsText;

export function DevelopmentPlanMenuCell({
	row,
}: RenderCellProps<DevelopmentPlanRow>) {
	const {
		handleEditOptionClick,
		handleApproveOptionClick,
		handleRejectOptionClick,
		handleDeleteOptionClick,
		id,
		startDate,
		status,
	} = row;

	const pickerButtonRef = useRef<HTMLButtonElement>(null);
	const [isOpenPopover, setIsOpenPopover] =
		useAddScrollListener(pickerButtonRef);

	const options: PickerButtonOption[] = useMemo(() => {
		const isApprovedButtonDisabled = status === DevelopmentPlanStatus.Approved;
		const isRejectButtonDisabled =
			status === DevelopmentPlanStatus.None ||
			status === DevelopmentPlanStatus.Rejected;

		return [
			{
				value: 'approve',
				addon: (
					<Button
						disabled={isApprovedButtonDisabled}
						onClick={handleApproveOptionClick}
					>
						{approveText}
					</Button>
				),
			},
			{
				value: 'reject',
				addon: (
					<Button
						color="secondary"
						disabled={isRejectButtonDisabled}
						onClick={handleRejectOptionClick}
					>
						{rejectText}
					</Button>
				),
			},
			{
				value: 'edit',
				addon: <Button color="secondary">{editText}</Button>,
				onClick: handleEditOptionClick,
			},
			{
				value: 'delete',
				addon: <Button color="secondary">{deleteText}</Button>,
				onClick: handleDeleteOptionClick,
			},
		];
	}, [status, id, startDate]);

	return (
		<PickerButton
			icon={<OptionDotsIcon />}
			popoverPosition="left-start"
			displayOnlyAddon
			options={options}
			isOpenPopover={isOpenPopover}
			setIsOpenPopover={setIsOpenPopover}
			ref={pickerButtonRef}
		/>
	);
}
