import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import {
	CurrencyItemInfo,
	CurrencyItemWrapper,
	ExchangeRateDisplay,
	HorizontalLine,
	RateValue,
} from './styles';
import { CurrencyItemProps } from './types';

export function CurrencyItem({
	code,
	name,
	baseAmount,
	rate,
	toCurrencyCode,
}: CurrencyItemProps) {
	const theme = useTheme();
	const sourceAmountLabel = `${baseAmount} ${code}`;

	return (
		<CurrencyItemWrapper>
			<CurrencyItemInfo>
				<Typography variant="m-600">{code}</Typography>
				<Typography variant="xs-400" color={theme.palette['text-gray']}>
					{name}
				</Typography>
			</CurrencyItemInfo>
			<ExchangeRateDisplay>
				<Typography variant="m-400">{sourceAmountLabel}</Typography>
				<HorizontalLine />
				<RateValue>
					<Typography variant="m-700" color={theme.palette['text-accent']}>
						{rate}
					</Typography>
					<Typography variant="m-400">{toCurrencyCode}</Typography>
				</RateValue>
			</ExchangeRateDisplay>
		</CurrencyItemWrapper>
	);
}
