export default function InfoPage() {
  return (
    <div className="min-h-screen bg-white dark:bg-slate-950">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <h1 className="text-4xl font-bold text-gray-900 dark:text-white mb-4">
          About ShumenTraffic
        </h1>
        <p className="text-gray-600 dark:text-gray-400 text-lg mb-12">
          Real-time bus tracking and schedule information for Shumen, Bulgaria.
        </p>

        {/* News Section */}
        <section className="mb-12">
          <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-6">
            Latest News
          </h2>
          <div className="space-y-6">
            {[1, 2, 3].map((i) => (
              <article
                key={i}
                className="p-6 bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700"
              >
                <div className="flex justify-between items-start mb-3">
                  <h3 className="text-lg font-semibold text-gray-900 dark:text-white">
                    News Item {i}
                  </h3>
                  <span className="text-sm text-gray-500 dark:text-gray-400">
                    {new Date(Date.now() - i * 86400000).toLocaleDateString()}
                  </span>
                </div>
                <p className="text-gray-600 dark:text-gray-400">
                  This is a placeholder for news content. Updates about bus service changes,
                  maintenance schedules, and other important information will be displayed here.
                </p>
              </article>
            ))}
          </div>
        </section>

        {/* General Info Section */}
        <section className="mb-12">
          <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-6">
            General Information
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="p-6 bg-blue-50 dark:bg-blue-900/20 rounded-lg border border-blue-200 dark:border-blue-800">
              <h3 className="text-lg font-semibold text-blue-900 dark:text-blue-100 mb-3">
                Operating Hours
              </h3>
              <p className="text-blue-800 dark:text-blue-200">
                Monday - Friday: 5:00 AM - 11:00 PM
                <br />
                Saturday: 6:00 AM - 10:00 PM
                <br />
                Sunday: 7:00 AM - 9:00 PM
              </p>
            </div>

            <div className="p-6 bg-green-50 dark:bg-green-900/20 rounded-lg border border-green-200 dark:border-green-800">
              <h3 className="text-lg font-semibold text-green-900 dark:text-green-100 mb-3">
                Ticket Information
              </h3>
              <p className="text-green-800 dark:text-green-200">
                Single Ticket: 1.50 BGN
                <br />
                Day Pass: 5.00 BGN
                <br />
                Monthly Pass: 80.00 BGN
              </p>
            </div>

            <div className="p-6 bg-orange-50 dark:bg-orange-900/20 rounded-lg border border-orange-200 dark:border-orange-800">
              <h3 className="text-lg font-semibold text-orange-900 dark:text-orange-100 mb-3">
                Contact Us
              </h3>
              <p className="text-orange-800 dark:text-orange-200">
                Email: info@shumentraffic.bg
                <br />
                Phone: +359 (0) 888 123 456
                <br />
                Address: Shumen, Bulgaria
              </p>
            </div>

            <div className="p-6 bg-purple-50 dark:bg-purple-900/20 rounded-lg border border-purple-200 dark:border-purple-800">
              <h3 className="text-lg font-semibold text-purple-900 dark:text-purple-100 mb-3">
                Service Updates
              </h3>
              <p className="text-purple-800 dark:text-purple-200">
                Follow us for real-time updates on service changes, delays, and special events.
                <br />
                Check back regularly for announcements.
              </p>
            </div>
          </div>
        </section>

        {/* Updates Section */}
        <section>
          <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-6">
            Recent Updates
          </h2>
          <div className="bg-gray-50 dark:bg-slate-900 rounded-lg border border-gray-200 dark:border-slate-700 p-6">
            <ul className="space-y-3">
              <li className="flex items-start gap-3">
                <span className="text-blue-600 dark:text-blue-400 font-bold mt-1">•</span>
                <span className="text-gray-700 dark:text-gray-300">
                  New real-time tracking system launched
                </span>
              </li>
              <li className="flex items-start gap-3">
                <span className="text-blue-600 dark:text-blue-400 font-bold mt-1">•</span>
                <span className="text-gray-700 dark:text-gray-300">
                  Mobile app now available for iOS and Android
                </span>
              </li>
              <li className="flex items-start gap-3">
                <span className="text-blue-600 dark:text-blue-400 font-bold mt-1">•</span>
                <span className="text-gray-700 dark:text-gray-300">
                  New bus lines added to service
                </span>
              </li>
              <li className="flex items-start gap-3">
                <span className="text-blue-600 dark:text-blue-400 font-bold mt-1">•</span>
                <span className="text-gray-700 dark:text-gray-300">
                  Improved schedule accuracy with GPS tracking
                </span>
              </li>
            </ul>
          </div>
        </section>
      </div>
    </div>
  );
}

