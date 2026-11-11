import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useTranslation } from 'react-i18next';

import { NoResultFoundContainer } from './styles';
import { NoResultsFoundRendererProps } from './types';

export function NoResultsFoundRenderer({
	rowHeight,
}: NoResultsFoundRendererProps) {
	const { t } = useTranslation('common', { keyPrefix: 'errors' });

	return (
		<NoResultFoundContainer rowHeight={rowHeight}>
			<Typography variant="m-400">{t('noResultsFound')}</Typography>
		</NoResultFoundContainer>
	);
}
