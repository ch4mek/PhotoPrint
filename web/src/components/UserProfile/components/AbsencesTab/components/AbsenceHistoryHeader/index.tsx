import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useTranslation } from 'react-i18next';

import { AbsenceHistoryControls } from './components/AbsenceHistoryControls';
import { AbsenceHistoryHeaderWrapper } from './styles';
import { AbsenceHistoryHeaderProps } from './types';

export function AbsenceHistoryHeader({
	absences,
	totalUsed,
}: AbsenceHistoryHeaderProps) {
	const { t } = useTranslation('profile', { keyPrefix: 'absence' });

	return (
		<AbsenceHistoryHeaderWrapper>
			<Typography variant="l-600">{t('history')}</Typography>
			<AbsenceHistoryControls {...{ absences, totalUsed }} />
		</AbsenceHistoryHeaderWrapper>
	);
}
