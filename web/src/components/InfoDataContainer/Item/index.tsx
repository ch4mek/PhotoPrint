import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { memo, useMemo } from 'react';

import { FixedTypography, ItemWrapper, NameOfFiledTypography } from './styles';
import { ItemProps } from './types';

function ItemInner({ nameOfField, value, secondValue }: ItemProps) {
	const trimedValue = useMemo(() => {
		if (typeof value === 'string') {
			return value.trim();
		}

		return value;
	}, [value]);

	const isHideTrimedValue = !value && secondValue;

	return (
		<ItemWrapper>
			{nameOfField && (
				<NameOfFiledTypography variant="s-400">
					{nameOfField}
				</NameOfFiledTypography>
			)}
			{!isHideTrimedValue && (
				<Tooltip text={trimedValue} useIfTextCut>
					<FixedTypography variant="m-400">
						{trimedValue || '-'}
					</FixedTypography>
				</Tooltip>
			)}

			{secondValue && (
				<Tooltip text={secondValue} useIfTextCut>
					<FixedTypography variant="m-400">{secondValue}</FixedTypography>
				</Tooltip>
			)}
		</ItemWrapper>
	);
}

export const Item = memo(ItemInner);
