import { Button } from '@mms/mms-ui-library/components/Button';
import {
	PickerButton,
	PickerButtonOption,
} from '@mms/mms-ui-library/components/PickerButton';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { useCallback, useContext, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { TimesheetStatus } from '../../../styles';

import { MutateTimesheetStatusContext } from '@/pages/TimeSheet/context/mutateTimesheetStatusContext';
import { TimesheetStatusType } from '@/pages/TimeSheet/types';
import { getTimesheetStatus } from '@/pages/TimeSheet/utils/getTimesheetStatus';

import { TimesheetStatusWrapper } from './styles';

interface TimesheetManagerStatusProps {
	userId: number;
	status: TimesheetStatusType | null;
}

export function TimesheetManagerStatus({
	userId,
	status,
}: TimesheetManagerStatusProps) {
	const { mutate } = useContext(MutateTimesheetStatusContext);
	const { t: tButtons } = useTranslation('common', {
		keyPrefix: 'controlButtons',
	});
	const { t: tStatus } = useTranslation('timesheet');
	const isApproveButtonDisabled = useMemo(
		() => status === TimesheetStatusType.Approved,
		[status]
	);

	const isRejectButtonDisabled = useMemo(
		() => status === TimesheetStatusType.Rejected || !status,
		[status]
	);

	const getListItemContextMenuOption = useCallback(
		(): PickerButtonOption<number>[] => [
			{
				value: 'approve',
				addon: (
					<Button type="button" disabled={isApproveButtonDisabled}>
						{tButtons('approve')}
					</Button>
				),
				onClick: (id: number) => {
					mutate({
						userId: id,
						status: TimesheetStatusType.Approved,
					});
				},
			},
			{
				value: 'reject',
				addon: (
					<Button
						type="button"
						color="secondary"
						disabled={isRejectButtonDisabled}
					>
						{tButtons('reject')}
					</Button>
				),
				onClick: (id: number) => {
					mutate({
						userId: id,
						status: TimesheetStatusType.Rejected,
					});
				},
			},
		],
		[status]
	);

	const contextMenuOptions = useMemo(
		() => getListItemContextMenuOption(),
		[getListItemContextMenuOption]
	);

	return (
		<TimesheetStatusWrapper>
			<PickerButton
				icon={<OptionDotsIcon />}
				item={userId}
				displayOnlyAddon
				options={contextMenuOptions}
				popoverPosition="left-start"
			/>
			{status && (
				<TimesheetStatus variant={status}>
					{getTimesheetStatus(status, tStatus)}
				</TimesheetStatus>
			)}
		</TimesheetStatusWrapper>
	);
}
