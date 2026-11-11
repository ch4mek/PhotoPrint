import { Dispatch, SetStateAction } from 'react';

export interface UseUpdateCategoryFormProps {
	activeCategoryId: null | number;
}

export interface UseDeleteCategoryForm {
	setActiveCategoryId: Dispatch<SetStateAction<null | number>>;
	setActiveTagId: Dispatch<SetStateAction<null | number>>;
	activeCategoryId: null | number;
}
