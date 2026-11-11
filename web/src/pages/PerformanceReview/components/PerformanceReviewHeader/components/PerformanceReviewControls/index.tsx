import { Switch } from '@mms/mms-ui-library/components/Switch';

import { createSelector } from '@/stores/helpers';
import { usePerformanceReviewStore } from '@/stores/performanceReview/store';

import { CONTROL_BAR_TITLE } from './contants';
import { ControlBarTitle, ControlBarWrapper } from './styles';

export function PerformanceReviewControls() {
	const { isShowIssuedStatus, toggleIsShowIssuedStatus } =
		usePerformanceReviewStore(
			createSelector('isShowIssuedStatus', 'toggleIsShowIssuedStatus')
		);

	return (
		<ControlBarWrapper>
			<ControlBarTitle variant="m-400">{CONTROL_BAR_TITLE}</ControlBarTitle>
			<Switch
				checked={isShowIssuedStatus}
				onChange={toggleIsShowIssuedStatus}
			/>
		</ControlBarWrapper>
	);
}
