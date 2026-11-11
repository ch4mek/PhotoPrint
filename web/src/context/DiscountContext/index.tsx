import {
	createContext,
	PropsWithChildren,
	useContext,
	useMemo,
	useState,
} from 'react';

import { DiscountContextValues } from '@/context/DiscountContext/types';
import { useDebounce } from '@/hooks/useDebounce';
import { DiscountCategoryOrder } from '@/types/discountCategories';
import { DiscountOrder } from '@/types/discounts';

const DiscountContext = createContext({} as DiscountContextValues);

export function DiscountProvider({ children }: PropsWithChildren) {
	const [activeCategoryId, setActiveCategoryId] = useState<number | null>(null);
	const [activeTagId, setActiveTagId] = useState<number | null>(null);
	const [activeCityId, setActiveCityId] = useState<number | null>(null);
	const [searchValue, setSearchValue] = useState('');
	const [isMoveModeEnable, setIsMoveModeEnable] = useState(false);

	const [categoryOrderData, setCategoryOrderData] = useState<
		DiscountCategoryOrder[]
	>([]);
	const [discountOrderData, setDiscountOrderData] = useState<DiscountOrder[]>(
		[]
	);

	const debouncedSearchValue = useDebounce(searchValue, 300);

	const isMoveEnable = useMemo(
		() => !!(activeTagId || activeCategoryId || activeCityId || searchValue),
		[activeTagId, activeCategoryId, activeCityId, searchValue]
	);

	const discountContextValue = useMemo(
		() => ({
			activeCategoryId,
			setActiveCategoryId,
			activeTagId,
			setActiveTagId,
			activeCityId,
			setActiveCityId,
			debouncedSearchValue,
			setSearchValue,
			searchValue,
			setIsMoveModeEnable,
			isMoveModeEnable,
			isMoveEnable,
			categoryOrderData,
			discountOrderData,
			setCategoryOrderData,
			setDiscountOrderData,
		}),
		[
			activeCategoryId,
			activeCityId,
			activeTagId,
			debouncedSearchValue,
			searchValue,
			isMoveModeEnable,
			isMoveEnable,
			categoryOrderData,
			discountOrderData,
		]
	);

	return (
		<DiscountContext.Provider value={discountContextValue}>
			{children}
		</DiscountContext.Provider>
	);
}

export const useDiscountContext = () => useContext(DiscountContext);
