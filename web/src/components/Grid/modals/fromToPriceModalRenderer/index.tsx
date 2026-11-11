import { useTheme } from '@emotion/react';
import { CurrencyInput } from '@mms/mms-ui-library/components/CurrencyInput';
import { useTranslation } from 'react-i18next';

import { ControlButtons } from '../../components';
import { RenderModalProps } from '../../renderers';
import {
	FromToTimeModalInputsWrapper,
	FromHoursToHoursTimeModalWrapper,
	VerticalDivider,
} from '../fromHoursToHoursModalRenderer/styles';
import { FromToProps } from '../types';

import { FromToPriceModalProps } from './types';

export function FromToPriceModal({
	selection,
	setSelection,
	onReset,
	onApply,
	applyDisabled,
	resetDisabled,
	includeDecimalPart = true,
}: RenderModalProps<FromToProps<string>> & FromToPriceModalProps) {
	const theme = useTheme();
	const { t } = useTranslation('common', {
		keyPrefix: 'modals.fromToPriceModal',
	});
	const { from, to } = selection;

	const handleInputChange = (field: string) => (value: string) => {
		setSelection((prevSelection) => ({
			...prevSelection,
			[field]: value,
		}));
	};

	const fromHoursRenderValue = !from ? '' : String(from);
	const toHoursRenderValue = !to ? '' : String(to);

	const inputStylesConfig = {
		label: {
			background: theme.palette['background-white'],
		},
	};

	return (
		<FromHoursToHoursTimeModalWrapper>
			<FromToTimeModalInputsWrapper>
				<CurrencyInput
					size="fullWidth"
					label={t('from')}
					value={fromHoursRenderValue}
					onValueChange={handleInputChange('from')}
					onCurrencyChange={() => null}
					currency="$"
					currenciesOptions={['$']}
					hideCurrencyChange
					stylesConfig={inputStylesConfig}
					autoComplete="off"
					includeDecimalPart={includeDecimalPart}
				/>
				<VerticalDivider />
				<CurrencyInput
					size="fullWidth"
					label={t('to')}
					value={toHoursRenderValue}
					onValueChange={handleInputChange('to')}
					onCurrencyChange={() => null}
					currenciesOptions={['$']}
					currency="$"
					hideCurrencyChange
					stylesConfig={inputStylesConfig}
					autoComplete="off"
					includeDecimalPart={includeDecimalPart}
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
