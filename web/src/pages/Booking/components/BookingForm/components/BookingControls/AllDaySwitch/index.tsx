import { Switch } from '@mms/mms-ui-library/components/Switch';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { memo } from 'react';
import { useTranslation } from 'react-i18next';

import { AllDaySwitchWrapper } from './styled';
import { AllDaySwitchProps } from './types';

const ALL_DAYS_SWITCH_NAME = 'all-day-switch';

function AllDaySwitchInner({
	checked,
	isDisabled,
	handleChange,
}: AllDaySwitchProps) {
	const { t } = useTranslation('booking', {
		keyPrefix: 'form.repetitive',
	});

	return (
		<AllDaySwitchWrapper htmlFor={ALL_DAYS_SWITCH_NAME}>
			<Switch
				checked={checked && !isDisabled}
				onChange={handleChange}
				name={ALL_DAYS_SWITCH_NAME}
				disabled={isDisabled}
				id={ALL_DAYS_SWITCH_NAME}
			/>
			<Typography variant="s-400">{t('allDayLabel')}</Typography>
		</AllDaySwitchWrapper>
	);
}

export const AllDaySwitch = memo(AllDaySwitchInner);
