import { useTheme } from '@emotion/react';
import { Input } from '@mms/mms-ui-library/components/Input';
import { ChangeEvent } from 'react';

import { ControlButtons } from '../../components';
import { RenderModalProps } from '../../renderers';
import {
	FromHoursToHoursTimeModalWrapper,
	FromToTimeModalInputsWrapper,
	VerticalDivider,
} from '../fromHoursToHoursModalRenderer/styles';
import { FromToProps } from '../types';

import { ONLY_DIGITS_REG_EXP } from '@/constants/regexp';

export function FromYearsToYearsTimeModal({
	onApply,
	onReset,
	selection,
	setSelection,
	applyDisabled,
	resetDisabled,
}: RenderModalProps<FromToProps<string>>) {
	const theme = useTheme();

	const { from, to } = selection;

	const handleInputChange =
		(field: string) => (e: ChangeEvent<HTMLInputElement>) => {
			e.stopPropagation();

			const value = e.target.value;

			if (value !== '' && !value.match(ONLY_DIGITS_REG_EXP)) {
				return;
			}

			if (!value) {
				setSelection((prevSelection) => ({
					...prevSelection,
					[field]: null,
				}));

				return;
			}

			setSelection((prevSelection) => ({
				...prevSelection,
				[field]: value,
			}));
		};

	const fromYearsRenderValue = from === null ? '' : String(from);
	const toYearsRenderValue = to === null ? '' : String(to);

	const inputStylesConfig = {
		label: {
			background: theme.palette['background-white'],
		},
	};

	return (
		<FromHoursToHoursTimeModalWrapper>
			<FromToTimeModalInputsWrapper>
				<Input
					size="fullWidth"
					label="From, y"
					value={fromYearsRenderValue}
					onChange={handleInputChange('from')}
					stylesConfig={inputStylesConfig}
					autoComplete="off"
				/>
				<VerticalDivider />
				<Input
					size="fullWidth"
					label="To, y"
					value={toYearsRenderValue}
					onChange={handleInputChange('to')}
					stylesConfig={inputStylesConfig}
					autoComplete="off"
				/>
			</FromToTimeModalInputsWrapper>
			<ControlButtons
				onApply={onApply}
				onReset={onReset}
				applyDisabled={applyDisabled}
				resetDisabled={resetDisabled}
			/>
		</FromHoursToHoursTimeModalWrapper>
	);
}
