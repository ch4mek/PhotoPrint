import { Button } from '@mms/mms-ui-library/components/Button';
import { useTranslation } from 'react-i18next';

import { CancelButtonProps } from './types';

export function CancelButton({ onClick, type, disabled }: CancelButtonProps) {
	const { t } = useTranslation('common', { keyPrefix: 'controlButtons' });
	const { t: tBooking } = useTranslation('booking', {
		keyPrefix: 'form.types',
	});

	return (
		<Button
			isFullWidth
			onClick={onClick}
			view="filled"
			color="secondary"
			disabled={disabled}
		>
			{`${t('cancel')} ${tBooking(type).toLowerCase()}`}
		</Button>
	);
}
