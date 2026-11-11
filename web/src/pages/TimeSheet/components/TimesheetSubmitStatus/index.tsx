import { Button } from '@mms/mms-ui-library/components/Button';
import { useQueryClient } from '@tanstack/react-query';
import { useContext } from 'react';
import { useTranslation } from 'react-i18next';

import { TimesheetStatusContext } from '../../context/timesheetStatusContext';
import { TimesheetStatusType } from '../../types';
import { getTimesheetStatus } from '../../utils/getTimesheetStatus';
import { TimesheetStatus } from '../styles';
import { isStatusEditable } from '../TimesheetEngineer/utils';

import { ProtectedView } from '@/components/ProtectedView';
import { TimesheetQueryKeys } from '@/constants/queriesKeys';
import { employees } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useUpdateTimesheetStatus } from '@/queries/timesheet/useUpdateUserTimesheetStatus';

import { SubmitWrapper } from './styles';
import { TimesheetSubmitStatusProps } from './types';

export function TimesheetSubmitStatus({ status }: TimesheetSubmitStatusProps) {
	const { id } = useAuth();
	const { t } = useTranslation('timesheet');
	const { month, year } = useContext(TimesheetStatusContext);
	const queryClient = useQueryClient();
	const { mutate } = useUpdateTimesheetStatus(id, month, year, {
		onSuccess: () => {
			queryClient.invalidateQueries({
				queryKey: [TimesheetQueryKeys.timesheetStatus],
			});
		},
	});
	const isStatusFetching = queryClient.isFetching({
		queryKey: TimesheetQueryKeys.timesheetStatus,
	});

	const handleSubmitStatus = () => {
		mutate({
			userId: id,
			status: TimesheetStatusType.UnderReview,
		});
	};

	const isDisabled = isStatusFetching !== 0 || !isStatusEditable(status);

	return (
		<ProtectedView roles={[...employees]}>
			<SubmitWrapper>
				{status && (
					<TimesheetStatus variant={status} isHeaderStatus>
						{getTimesheetStatus(status, t)}
					</TimesheetStatus>
				)}
				<Button
					type="submit"
					isFullWidth={false}
					onClick={handleSubmitStatus}
					disabled={isDisabled}
				>
					{t('sendToReview')}
				</Button>
			</SubmitWrapper>
		</ProtectedView>
	);
}
