import { useCallback } from 'react';

import { FilterTag } from '@/components/FilterTag';
import { useDiscountContext } from '@/context/DiscountContext';
import { CitiesTagListWrapper } from '@/pages/Discounts/components/DiscountsSection/components/DiscountControls/components/CitiesTagList/styles';
import { useGetDiscountCities } from '@/queries/discounts/useGetDiscountCities';

export function CitiesTagList() {
	const { setActiveCityId, activeCityId, isMoveModeEnable } =
		useDiscountContext();
	const { data } = useGetDiscountCities();

	const handleFilterTagClick = useCallback(
		(id: number) => {
			if (activeCityId === id) {
				setActiveCityId(null);

				return;
			}

			setActiveCityId(id);
		},
		[activeCityId]
	);

	return (
		<CitiesTagListWrapper>
			{data &&
				data.map(({ cityId, name }) => (
					<FilterTag
						key={cityId}
						id={cityId}
						title={name}
						isSelected={activeCityId === cityId}
						onClick={handleFilterTagClick}
						disabled={isMoveModeEnable}
					/>
				))}
		</CitiesTagListWrapper>
	);
}
