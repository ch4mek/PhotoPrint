import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useTranslation } from 'react-i18next';

export const renderTotalHeaderCell = () => {
	const theme = useTheme();
	const { t } = useTranslation('common', {
		keyPrefix: 'columns',
	});

	return (
		<Typography
			align="center"
			variant="s-600"
			color={theme.palette['primary-6']}
		>
			{t('total')}
		</Typography>
	);
};
