import { Typography } from '@mms/mms-ui-library/components/Typography';

import { AbsenceHistoryControls } from './components/AbsenceHistoryControls';
import { HISTORY_HEADER_TITLE } from './constants';
import { AbsenceHistoryHeaderWrapper } from './styles';
import { AbsenceHistoryHeaderProps } from './types';

export function AbsenceHistoryHeader({
	absences,
	totalUsed,
}: AbsenceHistoryHeaderProps) {
	return (
		<AbsenceHistoryHeaderWrapper>
			<Typography variant="l-600">{HISTORY_HEADER_TITLE}</Typography>
			<AbsenceHistoryControls {...{ absences, totalUsed }} />
		</AbsenceHistoryHeaderWrapper>
	);
}
