import {
	createContext,
	PropsWithChildren,
	useCallback,
	useContext,
	useMemo,
	useState,
} from 'react';

import { WikiContextValues } from '@/pages/Wiki/context/types';

const WikiContext = createContext<WikiContextValues>({} as WikiContextValues);

export function WikiProvider({ children }: PropsWithChildren) {
	const [isEditorReadOnly, setIsEditorReadOnly] = useState(true);

	const setToggleEditorReadOnly = useCallback(() => {
		setIsEditorReadOnly((prev) => !prev);
	}, []);

	const value: WikiContextValues = useMemo(
		() => ({
			isEditorReadOnly,
			setToggleEditorReadOnly,
			setIsEditorReadOnly,
		}),
		[isEditorReadOnly]
	);

	return <WikiContext.Provider value={value}>{children}</WikiContext.Provider>;
}

export const useWikiContext = () => useContext(WikiContext);
