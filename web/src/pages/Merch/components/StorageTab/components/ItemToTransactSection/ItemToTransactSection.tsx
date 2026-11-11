import { useTheme } from '@emotion/react';
import { Counter } from '@mms/mms-ui-library/components/Counter';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useFormikContext } from 'formik';
import { useMemo } from 'react';

import {
	MAX_AMOUNT_IN_TRANSACTION,
	MIN_AMOUNT_CLOTHING_IN_STORAGE_TRANSACTION,
	MIN_AMOUNT_IN_STORAGE_TRANSACTION,
} from '../../constants';

import { transactionTypes } from '@/forms/MerchStorageForm/constants';
import { MerchStorageFormValues } from '@/forms/MerchStorageForm/types';

import {
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
	isClothing,
	minAmountSizes,
	transactionType,
}: ItemToTransactSectionProps) {
	const theme = useTheme();
	const { values, setFieldValue, setFieldTouched } =
		useFormikContext<MerchStorageFormValues>();

	const { sizeCounts, warehouseId } = values;

	const selectedStorageMinAmountSizes = minAmountSizes?.filter(
		({ storageId }) => storageId === warehouseId
	);

	const handleSizeChange = (index: number) => (value: number) => {
		setFieldValue(`sizeCounts.${index}.count`, value);
		setFieldTouched(`sizeCounts.${index}.count`, true);
	};

	const showAllAvailableLabels =
		transactionType !== transactionTypes.Receipt.id;

	const showZeroAvailableBlock = useMemo(() => {
		if (!showAllAvailableLabels) {
			return false;
		}

		const allSizesHaveZeroAvailable = sizeCounts.every(({ sizeId }) => {
			const availableAmount = selectedStorageMinAmountSizes?.find(
				({ sizeId: minAmountSizeId }) => minAmountSizeId === sizeId
			)?.amount;

			return !availableAmount;
		});

		return allSizesHaveZeroAvailable;
	}, [showAllAvailableLabels, sizeCounts, selectedStorageMinAmountSizes]);

	return (
		<SizeSectionWrapper
			isColumnDirection={isClothing || showZeroAvailableBlock}
		>
			<NameWrapper>
				<Tooltip text={itemName} useIfTextCut>
					<ItemNameTypography variant="m-400">{itemName}</ItemNameTypography>
				</Tooltip>
			</NameWrapper>
			<SizeWrapper>
				{showZeroAvailableBlock ? (
					<Typography variant="s-400" color={theme.palette['text-gray']}>
						Available: 0
					</Typography>
				) : (
					sizeCounts.map(({ sizeId, count }, index) => {
						const availableAmount = selectedStorageMinAmountSizes?.find(
							({ sizeId: minAmountSizeId }) => minAmountSizeId === sizeId
						)?.amount;
						const showSizeItem =
							transactionType === transactionTypes.Receipt.id ||
							!isClothing ||
							availableAmount;

						if (!showSizeItem) {
							return null;
						}

						return (
							<SizeItemWrapper key={sizeId}>
								{isClothing && (
									<InfoItemSection>
										<Typography variant="m-400">{sizeId}</Typography>
										{showAllAvailableLabels && (
											<Typography
												variant="s-400"
												color={theme.palette['text-gray']}
											>
												Available: {availableAmount}
											</Typography>
										)}
									</InfoItemSection>
								)}
								<Counter
									minValue={
										isClothing
											? MIN_AMOUNT_CLOTHING_IN_STORAGE_TRANSACTION
											: MIN_AMOUNT_IN_STORAGE_TRANSACTION
									}
									maxValue={MAX_AMOUNT_IN_TRANSACTION}
									value={count}
									onChange={handleSizeChange(index)}
									editable
								/>
							</SizeItemWrapper>
						);
					})
				)}
			</SizeWrapper>
		</SizeSectionWrapper>
	);
}
