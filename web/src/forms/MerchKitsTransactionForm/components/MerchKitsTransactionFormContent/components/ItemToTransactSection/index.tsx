import { Counter } from '@mms/mms-ui-library/components/Counter';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useFormikContext } from 'formik';

import {
	MAX_AMOUNT,
	MIN_AMOUNT,
} from '@/forms/MerchKitsTransactionForm/constants';
import { MerchKitsTransactionFormValues } from '@/forms/MerchKitsTransactionForm/types';

import {
	AvailableSizes,
	InfoItemSection,
	ItemNameTypography,
	NameWrapper,
	SizeItemWrapper,
	SizeSectionWrapper,
	SizeWrapper,
} from './styles';
import { ItemToTransactSectionProps } from './types';

export function ItemToTransactSection({
	itemName,
	sizes,
	isReciept,
}: ItemToTransactSectionProps) {
	const { values, setFieldValue, setFieldTouched } =
		useFormikContext<MerchKitsTransactionFormValues>();

	const handleSizeChange = (sizeId: string) => (value: number) => {
		setFieldValue(
			'sizeCounts',
			values.sizeCounts.map((item) =>
				item.sizeId === sizeId ? { ...item, count: value } : item
			)
		);
		setFieldTouched('sizeCounts', true);
	};
	const visibleSizes = isReciept
		? sizes
		: sizes.filter(({ count }) => count > 0);

	return (
		<SizeSectionWrapper>
			<NameWrapper>
				<Tooltip text={itemName} useIfTextCut>
					<ItemNameTypography variant="m-400">{itemName}</ItemNameTypography>
				</Tooltip>
			</NameWrapper>
			<SizeWrapper>
				{visibleSizes.map(({ sizeId, count }) => {
					const counterValue =
						values.sizeCounts.find((item) => sizeId === item.sizeId)?.count ||
						MIN_AMOUNT;

					return (
						<SizeItemWrapper key={sizeId}>
							<InfoItemSection>
								<Typography variant="m-400">{sizeId}</Typography>
								{!isReciept && (
									<AvailableSizes variant="s-400">
										Available: {count}
									</AvailableSizes>
								)}
							</InfoItemSection>
							<Counter
								minValue={MIN_AMOUNT}
								maxValue={MAX_AMOUNT}
								value={counterValue}
								onChange={handleSizeChange(sizeId)}
								editable
							/>
						</SizeItemWrapper>
					);
				})}
			</SizeWrapper>
		</SizeSectionWrapper>
	);
}
