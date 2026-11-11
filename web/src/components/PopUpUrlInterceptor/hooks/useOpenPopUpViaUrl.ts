import { PopUpSearchParams } from '@/constants/searchParams';
import { useSearchParamsUrl } from '@/hooks/useSearchParamsUrl';

export const useOpenPopUpViaUrl = (param: PopUpSearchParams) => {
	const { setSearchParamsUrl } = useSearchParamsUrl();

	const handleOpenPopUpViaUrl: (id: string | number | null) => void = (id) => {
		setSearchParamsUrl(param, id == null ? '' : String(id));
	};

	return handleOpenPopUpViaUrl;
};
