import { useTheme } from '@emotion/react';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useMemo } from 'react';

import { useResize } from '@/hooks/useResize';
import { useGetExchangeRates } from '@/queries/rates/useGetExchangeRates';

import { CurrencyItem } from './components/CurrencyItem';
import { EXCHANGE_RATE_POPUP_TITLE } from './constants';
import { ContentWrapper, CurrenciesWrapper } from './styles';
import { ExchangeRatePopupProps } from './types';
import { getExchangeRatesRequestDate } from './utils/getExchangeRatesRequestDate';
import { getPopupHeaderText } from './utils/getPopupHeaderText';
import { getPopUpStylesConfig } from './utils/getPopUpStylesConfig';
import { mapExchangeRateToCurrencyItemProps } from './utils/mapExchangeRateToCurrencyItemProps';

export function ExchangeRatePopup({
	date,
	handleClose,
}: ExchangeRatePopupProps) {
	const theme = useTheme();
	const [, windowHeight] = useResize();
	const requestDate = getExchangeRatesRequestDate(date);
	const { data, isLoading } = useGetExchangeRates(requestDate);
	const exchangeRates = data?.exchangeRates ?? [];

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	return (
		<PopUp
			title={EXCHANGE_RATE_POPUP_TITLE}
			onClose={handleClose}
			type="info"
			showCloseButton
			showLoader={isLoading}
			stylesConfig={popUpStylesConfig}
		>
			<ContentWrapper>
				<Typography variant="s-400" color={theme.palette['text-gray']}>
					{getPopupHeaderText(data?.exchangeRateDate ?? null)}
				</Typography>
				<CurrenciesWrapper>
					{exchangeRates.map((exchangeRate) => (
						<CurrencyItem
							key={exchangeRate.fromCurrency.id}
							{...mapExchangeRateToCurrencyItemProps(exchangeRate)}
						/>
					))}
				</CurrenciesWrapper>
			</ContentWrapper>
		</PopUp>
	);
}
