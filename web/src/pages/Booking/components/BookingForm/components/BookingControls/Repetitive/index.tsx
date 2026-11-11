import { Switch } from '@mms/mms-ui-library/components/Switch';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import AccordionDetails from '@mui/material/AccordionDetails';
import { memo } from 'react';
import { useTranslation } from 'react-i18next';

import { RepeatWeeks } from '../RepeatWeeks';

import {
	StyledAccordionSummary,
	StyledAccordion,
	RepetativeSwitchContainer,
} from './styled';
import type { RepetitiveInnerProps } from './types';

const REPETATIVE_SWITCH_NAME = 'repeat-switch';

function RepetitiveInner({
	isChecked,
	isControlDisabled,
	weeksCount,
	handleWeeksChange,
	handleChange,
}: RepetitiveInnerProps) {
	const { t } = useTranslation('booking', {
		keyPrefix: 'form.repetitive',
	});

	return (
		<>
			<RepetativeSwitchContainer htmlFor={REPETATIVE_SWITCH_NAME}>
				<Switch
					checked={isChecked && !isControlDisabled}
					onChange={handleChange}
					name={REPETATIVE_SWITCH_NAME}
					id={REPETATIVE_SWITCH_NAME}
					disabled={isControlDisabled}
				/>
				<Typography variant="s-400">{t('repeatBookingLabel')}</Typography>
			</RepetativeSwitchContainer>

			<StyledAccordion
				expanded={isChecked && !isControlDisabled}
				disableGutters
				elevation={0}
			>
				<StyledAccordionSummary />
				<AccordionDetails>
					<RepeatWeeks value={weeksCount} onChange={handleWeeksChange} />
				</AccordionDetails>
			</StyledAccordion>
		</>
	);
}

export const Repetitive = memo(RepetitiveInner);
