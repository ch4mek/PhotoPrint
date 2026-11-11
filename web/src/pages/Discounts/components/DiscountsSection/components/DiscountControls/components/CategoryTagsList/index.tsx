import { useCallback } from 'react';

import { FilterTag } from '@/components/FilterTag';
import { FilterTagType } from '@/components/FilterTag/types';
import { useDiscountContext } from '@/context/DiscountContext';
import { CategoryTagsListWrapper } from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls/components/CategoryTagsList/styles';
import { useGetDiscountTagsByCategoryId } from '@/queries/discounts/tags/useGetDiscountTagsByCategoryId';

export function CategoryTagsList() {
	const { setActiveTagId, activeTagId, activeCategoryId, isMoveModeEnable } =
		useDiscountContext();
	const { data } = useGetDiscountTagsByCategoryId(activeCategoryId!, {
		enabled: activeCategoryId != null,
	});

	const handleFilterTagClick = useCallback(
		(id: number | string) => {
			if (activeTagId === id) {
				setActiveTagId(null);

				return;
			}

			setActiveTagId(+id);
		},
		[activeTagId]
	);

	if (!data) {
		return null;
	}

	return (
		<CategoryTagsListWrapper>
			{data.map(({ id, name }) => (
				<FilterTag
					key={id}
					id={id}
					title={name}
					isSelected={activeTagId === id}
					onClick={handleFilterTagClick}
					type={FilterTagType.Secondary}
					disabled={isMoveModeEnable}
				/>
			))}
		</CategoryTagsListWrapper>
	);
}
