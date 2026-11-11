export const getPortalContainer = (modalContainerSelector: string) => () => {
	const modalContainer = document.querySelector(
		modalContainerSelector
	) as HTMLElement;

	if (modalContainer === null) {
		return document.body;
	}

	return modalContainer;
};
