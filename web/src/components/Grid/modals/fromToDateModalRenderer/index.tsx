import { useTheme } from '@emotion/react';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { ControlButtons } from '../../components';
import { RenderModalProps } from '../../renderers';
import { FromToProps } from '../types';

import { DATE_RANGE_IS_INVALID } from '@/constants/errors';
import dayjs, { Dayjs } from '@/lib/dayjs';

import {
	DataWrapper,
	FromToDateModalInputsWrapper,
	FromToDateModalWrapper,
} from './styles';

export function FromToDateModal({
	onApply,
	onReset,
	selection,
	setSelection,
	applyDisabled,
	resetDisabled,
}: RenderModalProps<FromToProps<Date>>) {
	const { from, to } = selection;
	const { t } = useTranslation('common', { keyPrefix: 'placeholders' });
	const theme = useTheme();

	const datePickerStylesConfig = useMemo(
		() => ({
			input: {
				placeholderColor: `${theme.palette['text-gray']}`,
			},
		}),
		[theme]
	);

	const handleInputChange = (type: string) => (value: Dayjs | null) => {
		setSelection((prev) => ({ ...prev, [type]: value }));
	};

	const isFromDateCompleted = useMemo(
		() => !from || (from && dayjs(from).isValid()),
		[from]
	);
	const isToDateCompleted = useMemo(
		() => !to || (to && dayjs(to).isValid()),
		[to]
	);

	return (
		<FromToDateModalWrapper>
			<FromToDateModalInputsWrapper>
				<DataWrapper bottomSpace={16}>
					<DatePicker
						size="fullWidth"
						label={t('from')}
						value={dayjs(from)}
						calendarPosition="bottom-start"
						onChange={handleInputChange('from')}
						stylesConfig={datePickerStylesConfig}
						errorText={!isFromDateCompleted ? DATE_RANGE_IS_INVALID : ''}
					/>
				</DataWrapper>
				<DataWrapper bottomSpace={0}>
					<DatePicker
						size="fullWidth"
						label={t('to')}
						value={dayjs(to)}
						calendarPosition="bottom-start"
						onChange={handleInputChange('to')}
						stylesConfig={datePickerStylesConfig}
						errorText={!isToDateCompleted ? DATE_RANGE_IS_INVALID : ''}
					/>
				</DataWrapper>
			</FromToDateModalInputsWrapper>
			<ControlButtons
				onApply={onApply}
				onReset={onReset}
				applyDisabled={
					applyDisabled || !isFromDateCompleted || !isToDateCompleted
				}
				resetDisabled={resetDisabled}
			/>
		</FromToDateModalWrapper>
	);
}
