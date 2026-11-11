import i18next from 'i18next';

import enBooking from '@/i18n/locales/en/booking.json';
import enCommon from '@/i18n/locales/en/common.json';

const E2E_LOCALE = 'en';

const e2eI18n = i18next.createInstance();

e2eI18n.init({
	lng: E2E_LOCALE,
	fallbackLng: E2E_LOCALE,
	resources: {
		[E2E_LOCALE]: {
			booking: enBooking,
			common: enCommon,
		},
	},
	interpolation: {
		escapeValue: false,
	},
});

type E2eTranslate = (key: string, options?: Record<string, unknown>) => string;

const createNamespaceTranslate = (namespace: string): E2eTranslate => {
	const translate = e2eI18n.t as (
		key: string,
		options?: Record<string, unknown>
	) => string;

	return (key, options) =>
		translate(key, {
			lng: E2E_LOCALE,
			ns: namespace,
			...(options ?? {}),
		});
};

const bookingT = createNamespaceTranslate('booking');
const commonT = createNamespaceTranslate('common');

export const BOOK_DESK_BUTTON_TEXT = bookingT('actions.book', {
	resource: bookingT('labels.deskAccusative'),
});
export const BLOCK_DESK_BUTTON_TEXT = bookingT('actions.blockDesk');
export const DESK_BOOKED_TOAST_TEXT = bookingT('toasts.deskBooked');
export const ALREADY_HAVE_BOOKING_WARNING_TEXT = bookingT(
	'errors.alreadyHaveBooking'
);
export const GUEST_ALREADY_BOOKED_WARNING_TEXT = bookingT(
	'errors.guestAlreadyBooked'
);
export const UNBLOCK_DESK_CONFIRM_TEXT = bookingT('dialogs.unblock.confirm');
export const BOOKINGS_TAB_TEXT = bookingT('blockTabs.bookings');
export const DETAILS_TAB_TEXT = bookingT('blockTabs.details');
export const GUESTS_MODE_TEXT = bookingT('form.modes.guest');
export const TO_DATE_PLACEHOLDER = commonT('placeholders.to');
export const YES_BUTTON_TEXT = commonT('modals.controls.yes');
