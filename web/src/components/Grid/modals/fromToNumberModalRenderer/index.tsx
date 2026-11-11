import { useTheme } from '@emotion/react';
import { Input } from '@mms/mms-ui-library/components/Input';
import { ChangeEvent } from 'react';

import { ControlButtons } from '../../components';
import { RenderModalProps } from '../../renderers';
import {
	FromToTimeModalInputsWrapper,
	FromHoursToHoursTimeModalWrapper,
	VerticalDivider,
} from '../fromHoursToHoursModalRenderer/styles';
import { FromToProps } from '../types';

import { ONLY_DIGITS_REG_EXP } from '@/constants/regexp';

export function FromToNumberModal({
	selection,
	setSelection,
	onReset,
	onApply,
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

	const fromNumberRenderValue = !from ? '' : String(from);
	const toNumberRenderValue = !to ? '' : String(to);

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
					label="From"
					name="from"
					value={fromNumberRenderValue}
					onChange={handleInputChange('from')}
					stylesConfig={inputStylesConfig}
				/>
				<VerticalDivider />
				<Input
					size="fullWidth"
					label="To"
					name="to"
					value={toNumberRenderValue}
					onChange={handleInputChange('to')}
					stylesConfig={inputStylesConfig}
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
