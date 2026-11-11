import { Button } from '@mms/mms-ui-library/components/Button';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useTranslation } from 'react-i18next';

import { PAYSLIP_DESCRIPTION_LINK } from './constants';
import { PayslipsHeaderWrapper } from './styles';

export function PayslipsHeader() {
	const handleViewDescription = () => {
		window.open(PAYSLIP_DESCRIPTION_LINK, '_blank');
	};

	const { t } = useTranslation('profile', { keyPrefix: 'salaryTab' });

	return (
		<PayslipsHeaderWrapper>
			<Typography variant="l-600">{t('payslips')}</Typography>
			<Button
				isFullWidth={false}
				color="secondary"
				onClick={handleViewDescription}
			>
				{t('viewPayslipDescription')}
			</Button>
		</PayslipsHeaderWrapper>
	);
}
