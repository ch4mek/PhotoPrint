import { useTheme } from '@emotion/react';
import { Button } from '@mms/mms-ui-library/components/Button';
import { CurrencyInput } from '@mms/mms-ui-library/components/CurrencyInput';
import { useCallback } from 'react';

import {
	KPI_COLOR_KEYS,
	PERCENT_SYMBOL,
	SAVE_KPI_COLORS_BUTTON_LABEL,
} from '../../constants';
import {
	KPIColorPercentInputWrapper,
	KPIPercentItem,
	KPITabFormWrapper,
} from '../../styles';
import { KPIColorsFormFields } from '../../types';
import { getKPIColorIntervalProperty } from '../../utils/getKPIColorIntervalProperty';

import {
	ColorCircle,
	ColorCircleCellContentWrapper,
} from '@/components/Grid/renderers/cell/cellWithColorCircleRenderer/styles';
import { Loader } from '@/components/Loader';
import { MAX_NUMBER_TYPE_1_DIGITS } from '@/forms/schemas/constants';

import { KPITabFormProps } from './types';

export const KPITabForm = ({
	formikProps,
	isFetching,
	isError,
	isUpdatingKPIColors,
}: KPITabFormProps) => {
	const theme = useTheme();

	const {
		values,
		errors,
		touched,
		handleSubmit,
		setFieldValue,
		setFieldTouched,
	} = formikProps;

	const handleFieldValueChange = useCallback(
		(fieldName: KPIColorsFormFields) => (value: string) => {
			setFieldValue(getKPIColorIntervalProperty(fieldName), value);
			setFieldTouched(fieldName, true);
		},
		[setFieldValue]
	);

	const isSaveButtonDisabled = !(errors.minColor || errors.middleColor);

	const isAnyFieldTouched = Boolean(touched.minColor || touched.middleColor);

	return (
		<Loader isLoading={isFetching}>
			<KPITabFormWrapper>
				<KPIPercentItem>
					<ColorCircleCellContentWrapper>
						<ColorCircle color={theme.palette[KPI_COLOR_KEYS.minColor]} />
						{values.minColor.name}
					</ColorCircleCellContentWrapper>
					<KPIColorPercentInputWrapper>
						<CurrencyInput
							currency={PERCENT_SYMBOL}
							onValueChange={handleFieldValueChange('minColor')}
							value={values.minColor.rightIntervalBorder}
							label=""
							disabledCurrencyChange
							includeDecimalPart={false}
							maxLength={MAX_NUMBER_TYPE_1_DIGITS}
							error={
								Boolean(errors.minColor) || (isError && !isAnyFieldTouched)
							}
							errorText={String(errors.minColor ?? '')}
						/>
					</KPIColorPercentInputWrapper>
				</KPIPercentItem>
				<KPIPercentItem>
					<ColorCircleCellContentWrapper>
						<ColorCircle color={theme.palette[KPI_COLOR_KEYS.middleColor]} />
						{values.middleColor.name}
					</ColorCircleCellContentWrapper>
					<KPIColorPercentInputWrapper>
						<CurrencyInput
							currency={PERCENT_SYMBOL}
							onValueChange={handleFieldValueChange('middleColor')}
							value={values.middleColor.rightIntervalBorder}
							label=""
							disabledCurrencyChange
							includeDecimalPart={false}
							maxLength={MAX_NUMBER_TYPE_1_DIGITS}
							error={
								Boolean(errors.middleColor) || (isError && !isAnyFieldTouched)
							}
							errorText={String(errors.middleColor ?? '')}
						/>
					</KPIColorPercentInputWrapper>
				</KPIPercentItem>
				<KPIPercentItem>
					<ColorCircleCellContentWrapper>
						<ColorCircle color={theme.palette[KPI_COLOR_KEYS.maxColor]} />
						{values.maxColor.name}
					</ColorCircleCellContentWrapper>
				</KPIPercentItem>
				<Button
					disabled={!isSaveButtonDisabled || isUpdatingKPIColors}
					onClick={() => handleSubmit()}
				>
					{SAVE_KPI_COLORS_BUTTON_LABEL}
				</Button>
			</KPITabFormWrapper>
		</Loader>
	);
};
