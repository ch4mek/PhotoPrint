import { Typography } from '@mms/mms-ui-library/components/Typography';
import { TFunction } from 'i18next';

export const generateWarningBlockMessage = (
	seatName: string,
	t: TFunction<'booking'>
) => (
	<>
		{t('warnings.cannotBookPrefix')}{' '}
		<Typography variant="m-600">{seatName} </Typography>
		{t('warnings.isBlocked')}
	</>
);

export const generateWarningBookMessage = (
	seatName: string,
	period: string,
	t: TFunction<'booking'>
) => (
	<>
		{t('warnings.cannotBookPrefix')} {seatName}{' '}
		{t('warnings.isBookedForPeriod', { period })}
	</>
);
